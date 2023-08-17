using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class OpenGallry : MonoBehaviour
{
    private Texture2D m_Texture;
    public Image imageSelect;

    private string filename;
    public void OnButtonClick()
    {
        // Lấy ảnh từ thư viện
        NativeGallery.GetImageFromGallery((path) => {
            // Đọc dữ liệu ảnh từ đường dẫn
            byte[] data = System.IO.File.ReadAllBytes(path);

            // Tạo texture từ dữ liệu ảnh
            m_Texture = new Texture2D(2, 2);
            m_Texture.LoadImage(data);

            // Hiển thị ảnh lên màn hình
            SetImage(m_Texture);
            UpdateProfile.Ins.Texture2D = m_Texture;
            filename = Path.GetFileName(path);
            UpdateProfile.Ins.filenameAvatar = filename;
        });
    }
    public void SetImage(Texture2D texture)
    {
        // Tạo một Sprite mới từ texture được truyền vào
        Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one / 2f);

        // Gán Sprite mới vào component Image
        imageSelect.sprite = newSprite;
    }
}
